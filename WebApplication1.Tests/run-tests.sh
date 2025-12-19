#!/bin/bash

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
BOLD='\033[1m'
NC='\033[0m' # No Color

# Unicode symbols
CHECK="✓"
CROSS="✗"
ARROW="→"
STAR="★"

echo ""
echo -e "${BOLD}${CYAN}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${BOLD}${CYAN}║          WebApplication1 - Test Suite Runner              ║${NC}"
echo -e "${BOLD}${CYAN}╚════════════════════════════════════════════════════════════╝${NC}"
echo ""

echo -e "${BLUE}${ARROW} Running all tests...${NC}"
echo ""

# Run tests and capture output with normal verbosity
TEST_OUTPUT=$(dotnet test --verbosity normal --nologo 2>&1)
EXIT_CODE=$?

# Parse results
PASSED=$(echo "$TEST_OUTPUT" | grep -oE "Passed:[[:space:]]*[0-9]+" | tail -1 | grep -oE "[0-9]+")
FAILED=$(echo "$TEST_OUTPUT" | grep -oE "Failed:[[:space:]]*[0-9]+" | tail -1 | grep -oE "[0-9]+")
TOTAL=$(echo "$TEST_OUTPUT" | grep -oE "Total tests:[[:space:]]*[0-9]+" | tail -1 | grep -oE "[0-9]+")
DURATION=$(echo "$TEST_OUTPUT" | grep -oE "Total time:[[:space:]]*[0-9.]+ [A-Za-z]+" | tail -1 | sed 's/Total time:[[:space:]]*//')

# Default values if parsing fails
PASSED=${PASSED:-0}
FAILED=${FAILED:-0}
TOTAL=${TOTAL:-0}

# Calculate pass rate
if [ "$TOTAL" -gt 0 ]; then
    PASS_RATE=$((PASSED * 100 / TOTAL))
else
    PASS_RATE=0
fi

echo ""
echo -e "${BOLD}${MAGENTA}┌─────────────────────────────────────────────────────────────────┐${NC}"
echo -e "${BOLD}${MAGENTA}│                        TEST SUMMARY                             │${NC}"
echo -e "${BOLD}${MAGENTA}├─────────────────────────────────────────────────────────────────┤${NC}"

# Passed tests
if [ "$PASSED" -gt 0 ]; then
    printf "${MAGENTA}│${NC} ${GREEN}${CHECK} Passed:${NC}  ${BOLD}${GREEN}%-53s${NC} ${MAGENTA}│${NC}\n" "${PASSED}"
else
    printf "${MAGENTA}│${NC} ${GREEN}${CHECK} Passed:${NC}  %-53s ${MAGENTA}│${NC}\n" "0"
fi

# Failed tests
if [ "$FAILED" -gt 0 ]; then
    printf "${MAGENTA}│${NC} ${RED}${CROSS} Failed:${NC}  ${BOLD}${RED}%-53s${NC} ${MAGENTA}│${NC}\n" "${FAILED}"
else
    printf "${MAGENTA}│${NC} ${RED}${CROSS} Failed:${NC}  %-53s ${MAGENTA}│${NC}\n" "0"
fi

# Total tests
printf "${MAGENTA}│${NC} ${CYAN}${STAR} Total:${NC}   ${BOLD}%-53s${NC} ${MAGENTA}│${NC}\n" "${TOTAL}"

# Duration
if [ -n "$DURATION" ]; then
    printf "${MAGENTA}│${NC} ${BLUE}⏱  Duration:${NC} %-53s ${MAGENTA}│${NC}\n" "${DURATION}"
fi

echo -e "${BOLD}${MAGENTA}├─────────────────────────────────────────────────────────────────┤${NC}"

# Pass rate with color coding
if [ "$PASS_RATE" -eq 100 ]; then
    printf "${MAGENTA}│${NC} ${BOLD}Pass Rate:${NC}  ${GREEN}%-53s${NC} ${MAGENTA}│${NC}\n" "${PASS_RATE}% 🎉"
elif [ "$PASS_RATE" -ge 80 ]; then
    printf "${MAGENTA}│${NC} ${BOLD}Pass Rate:${NC}  ${YELLOW}%-53s${NC} ${MAGENTA}│${NC}\n" "${PASS_RATE}% ⚡"
else
    printf "${MAGENTA}│${NC} ${BOLD}Pass Rate:${NC}  ${RED}%-53s${NC} ${MAGENTA}│${NC}\n" "${PASS_RATE}% ⚠️"
fi

echo -e "${BOLD}${MAGENTA}└─────────────────────────────────────────────────────────────────┘${NC}"
echo ""

# Show detailed results by test class
echo -e "${BOLD}${CYAN}═══════════════════════════════════════════════════════════════════${NC}"
echo -e "${BOLD}${CYAN}                    TEST RESULTS BY CLASS                          ${NC}"
echo -e "${BOLD}${CYAN}═══════════════════════════════════════════════════════════════════${NC}"
echo ""

# WHITE BOX TESTS
echo -e "${BOLD}${YELLOW}┌─────────────────────────────────────────────────────────────────┐${NC}"
echo -e "${BOLD}${YELLOW}│                      WHITE BOX TESTS                            │${NC}"
echo -e "${BOLD}${YELLOW}└─────────────────────────────────────────────────────────────────┘${NC}"

# Count tests by class name (matching "Passed" lines with class names)
CART_WB=$(echo "$TEST_OUTPUT" | grep "Passed.*CartControllerWhiteBoxTests\|CartControllerWhiteBoxTests.*Passed" | wc -l | tr -d ' ')
PROD_WB=$(echo "$TEST_OUTPUT" | grep "Passed.*ProductControllerWhiteBoxTests\|ProductControllerWhiteBoxTests.*Passed" | wc -l | tr -d ' ')
SHOP_WB=$(echo "$TEST_OUTPUT" | grep "Passed.*ShopControllerWhiteBoxTests\|ShopControllerWhiteBoxTests.*Passed" | wc -l | tr -d ' ')
VM_WB=$(echo "$TEST_OUTPUT" | grep "Passed.*ViewModelValidationWhiteBoxTests\|ViewModelValidationWhiteBoxTests.*Passed" | wc -l | tr -d ' ')
MISC_WB=$(echo "$TEST_OUTPUT" | grep "Passed.*MiscControllersWhiteBoxTests\|MiscControllersWhiteBoxTests.*Passed" | wc -l | tr -d ' ')

echo -e "${GREEN}${CHECK}${NC} ${BOLD}CartControllerWhiteBoxTests${NC} - ${GREEN}${CART_WB} tests${NC}"
echo -e "${GREEN}${CHECK}${NC} ${BOLD}ProductControllerWhiteBoxTests${NC} - ${GREEN}${PROD_WB} tests${NC}"
echo -e "${GREEN}${CHECK}${NC} ${BOLD}ShopControllerWhiteBoxTests${NC} - ${GREEN}${SHOP_WB} tests${NC}"
echo -e "${GREEN}${CHECK}${NC} ${BOLD}ViewModelValidationWhiteBoxTests${NC} - ${GREEN}${VM_WB} tests${NC}"
echo -e "${GREEN}${CHECK}${NC} ${BOLD}MiscControllersWhiteBoxTests${NC} - ${GREEN}${MISC_WB} tests${NC}"

WB_TOTAL=$((CART_WB + PROD_WB + SHOP_WB + VM_WB + MISC_WB))
echo -e "${CYAN}   ─────────────────────────────────────────────────────────────${NC}"
echo -e "${BOLD}   White Box Total: ${GREEN}${WB_TOTAL} tests${NC}"
echo ""

# BLACK BOX TESTS
echo -e "${BOLD}${BLUE}┌─────────────────────────────────────────────────────────────────┐${NC}"
echo -e "${BOLD}${BLUE}│                      BLACK BOX TESTS                            │${NC}"
echo -e "${BOLD}${BLUE}└─────────────────────────────────────────────────────────────────┘${NC}"

PROD_BB=$(echo "$TEST_OUTPUT" | grep "Passed.*ProductControllerBlackBoxTests\|ProductControllerBlackBoxTests.*Passed" | wc -l | tr -d ' ')
SHOP_BB=$(echo "$TEST_OUTPUT" | grep "Passed.*ShopControllerBlackBoxTests\|ShopControllerBlackBoxTests.*Passed" | wc -l | tr -d ' ')
REG_BB=$(echo "$TEST_OUTPUT" | grep "Passed.*RegisterViewModelBlackBoxTests\|RegisterViewModelBlackBoxTests.*Passed" | wc -l | tr -d ' ')
LOGIN_BB=$(echo "$TEST_OUTPUT" | grep "Passed.*LoginViewModelBlackBoxTests\|LoginViewModelBlackBoxTests.*Passed" | wc -l | tr -d ' ')

echo -e "${GREEN}${CHECK}${NC} ${BOLD}ProductControllerBlackBoxTests${NC} - ${GREEN}${PROD_BB} tests${NC}"
echo -e "${GREEN}${CHECK}${NC} ${BOLD}ShopControllerBlackBoxTests${NC} - ${GREEN}${SHOP_BB} tests${NC}"
echo -e "${GREEN}${CHECK}${NC} ${BOLD}RegisterViewModelBlackBoxTests${NC} - ${GREEN}${REG_BB} tests${NC}"
echo -e "${GREEN}${CHECK}${NC} ${BOLD}LoginViewModelBlackBoxTests${NC} - ${GREEN}${LOGIN_BB} tests${NC}"

BB_TOTAL=$((PROD_BB + SHOP_BB + REG_BB + LOGIN_BB))
echo -e "${CYAN}   ─────────────────────────────────────────────────────────────${NC}"
echo -e "${BOLD}   Black Box Total: ${GREEN}${BB_TOTAL} tests${NC}"
echo ""

# Final status
if [ "$EXIT_CODE" -eq 0 ]; then
    echo -e "${BOLD}${GREEN}╔════════════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${GREEN}║                      ALL TESTS PASSED! 🎉                          ║${NC}"
    echo -e "${BOLD}${GREEN}╠════════════════════════════════════════════════════════════════════╣${NC}"
    printf "${GREEN}║${NC}   White Box: ${BOLD}${WB_TOTAL}${NC} tests    |    Black Box: ${BOLD}${BB_TOTAL}${NC} tests    |    Total: ${BOLD}${TOTAL}${NC}  ${GREEN}║${NC}\n"
    echo -e "${BOLD}${GREEN}╚════════════════════════════════════════════════════════════════════╝${NC}"
else
    echo -e "${BOLD}${RED}╔════════════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${RED}║              SOME TESTS FAILED (${FAILED}/${TOTAL})                            ║${NC}"
    echo -e "${BOLD}${RED}╚════════════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${YELLOW}${ARROW} Failed Tests:${NC}"
    echo "$TEST_OUTPUT" | grep "Failed " | head -20
fi

echo ""

exit $EXIT_CODE
